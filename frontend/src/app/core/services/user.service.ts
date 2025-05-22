import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable()
export class UserService {
  private endpoint = 'user';
  constructor(private httpClient: HttpClient) {}

  get(id: string): Observable<any> {
    return this.httpClient.get<any>(`${this.endpoint}/${id}`);
  }

  getByUserName(userName: string): Observable<any> {
    return this.httpClient.get<any>(`${this.endpoint}/autocomplete?userName=${userName}`);
  }

  login(username: string, password: string): Observable<any> {
    return this.httpClient.post<any>(`${this.endpoint}/login`, {
      username,
      password,
    });
  }

  edit(id: string, user: any): Observable<any> {
    return this.httpClient.put<any>(`${this.endpoint}?userId=${id}`, user);
  }

  register(user: any): Observable<any> {
    return this.httpClient.post<any>(`${this.endpoint}`, user);
  }
}
