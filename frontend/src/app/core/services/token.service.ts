import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { TokenModel } from '../../auth/models/token.model';

@Injectable()
export class TokenService {
  private key = `${environment.appVersion}-${environment.USERDATA_KEY}`;;

  constructor() {}

  set store(token: string) {
    window.sessionStorage.setItem(this.key, token);
  }

  get get(): TokenModel | null {
    const token = window.sessionStorage.getItem(this.key);

    if (!token) return null;
    return this.decrypt(token);
  }

  get authenticated(): boolean {
    const token = window.sessionStorage.getItem(this.key);

    return token != null;
  }

  get token(): any {
    return window.sessionStorage.getItem(this.key);
  }

  private decrypt(token: string): TokenModel {
    return new TokenModel(token);
  }
}