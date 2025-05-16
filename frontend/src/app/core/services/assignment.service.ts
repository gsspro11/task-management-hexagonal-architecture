import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable()
export class AssignmentService {
  private endpoint = 'assignment';
  constructor(private httpClient: HttpClient) {}

  get(id: string): Observable<any> {
    return this.httpClient.get<any>(`${this.endpoint}?userId=${id}`);
  }

  edit(id: string, assignment: any): Observable<any> {
    return this.httpClient.put<any>(`${this.endpoint}?id=${id}`, assignment);
  }

  register(assignment: any): Observable<any> {
    return this.httpClient.post<any>(`${this.endpoint}`, assignment);
  }
}
