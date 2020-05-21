import { Injectable } from '@angular/core';

@Injectable()
export class AuthService {

  get isLoggedIn(): boolean {
    const authToken = localStorage.getItem('access_token');
    return (authToken !== null) ? true : false;
  }

  getToken() {
    return localStorage.getItem('access_token');
  }
}
