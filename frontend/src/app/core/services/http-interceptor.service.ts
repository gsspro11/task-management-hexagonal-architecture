import {
  HttpErrorResponse,
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, Observable, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import { TokenService } from './token.service';

@Injectable()
export class HttpInterceptorService implements HttpInterceptor {
  constructor(private tokenService: TokenService) {}

  intercept(
    req: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    const update: any = {};

    update.url = `${environment.apiUrl}/${req.url}`;

    if (this.tokenService.authenticated)
      update.setHeaders = {
        Authorization: `Bearer ${this.tokenService.token}`,
      };

    req = req.clone(update);

    return next.handle(req).pipe(
      catchError((error: HttpErrorResponse) => {
        let errorMessage = 'Something unexpected happened.';

        if (error.error.message)
          return throwError(() => new Error(error.error.message));

        if (error.error[0].message) {
          errorMessage = '';

          error.error.forEach((x: { message: string; }) => {
            errorMessage += x.message;
          });
        }

        return throwError(() => new Error(errorMessage));
      })
    );
  }
}
