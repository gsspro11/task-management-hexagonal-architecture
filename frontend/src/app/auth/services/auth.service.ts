import { Injectable, OnDestroy } from '@angular/core';
import { Observable, BehaviorSubject, of, Subscription } from 'rxjs';
import { map, catchError, switchMap, finalize } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { Router } from '@angular/router';
import { UserService } from '../../core/services/user.service';

@Injectable({
  providedIn: 'root',
})
export class AuthService implements OnDestroy {
  // private fields
  private unsubscribe: Subscription[] = []; // Read more: => https://brianflove.com/2016/12/11/anguar-2-unsubscribe-observables/
  private authLocalStorageToken = `${environment.appVersion}-${environment.USERDATA_KEY}`;

  // public fields
  currentUser$: Observable<any>;
  isLoading$: Observable<boolean>;
  currentUserSubject: BehaviorSubject<any>;
  isLoadingSubject: BehaviorSubject<boolean>;

  get currentUserValue(): any {
    return this.currentUserSubject.value;
  }

  set currentUserValue(user: any) {
    this.currentUserSubject.next(user);
  }

  constructor(private usuarioService: UserService, private router: Router) {
    this.isLoadingSubject = new BehaviorSubject<boolean>(false);
    this.currentUserSubject = new BehaviorSubject<any>(undefined);
    this.currentUser$ = this.currentUserSubject.asObservable();
    this.isLoading$ = this.isLoadingSubject.asObservable();
    const subscr = this.getUserByToken().subscribe();
    this.unsubscribe.push(subscr);
  }

  login(email: string, password: string): Observable<any> {
    this.isLoadingSubject.next(true);
    return this.usuarioService.login(email, password).pipe(
      map((auth: any) => {
        const result = this.setAuthFromLocalStorage(auth);
        return result;
      }),
      switchMap(() => this.getUserByToken()),
      catchError((error) => {
        console.log(error);
        return of(error);
      }),
      finalize(() => this.isLoadingSubject.next(false))
    );
  }

  logout() {
    sessionStorage.removeItem(this.authLocalStorageToken);
    this. getUserByToken();
    this.router.navigate(['/login'], {
      queryParams: {},
    });
  }

  getUserByToken(): Observable<any> {
    const token = this.getAuthFromLocalStorage();
    this.currentUserSubject = new BehaviorSubject<any>(token);
    return of(token);
  }

  //   registration(user: any): Observable<any> {
  //     return this.usuarioService.cadastrar(user);
  //   }

  setAuthFromLocalStorage(auth: any): boolean {
    // store auth authToken/refreshToken/epiresIn in local storage to keep user logged in between page refreshes
    if (auth && (auth.authToken || auth.token)) {
      sessionStorage.setItem(this.authLocalStorageToken, JSON.stringify(auth));
      return true;
    }
    return false;
  }

  private getAuthFromLocalStorage(): any {
    try {
      return sessionStorage.getItem(this.authLocalStorageToken);
    } catch (error) {
      console.error(error);
      return undefined;
    }
  }

  ngOnDestroy() {
    this.unsubscribe.forEach((sb) => sb.unsubscribe());
  }
}
