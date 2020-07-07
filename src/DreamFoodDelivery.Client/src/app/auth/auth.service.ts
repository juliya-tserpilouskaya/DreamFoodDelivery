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

  getTokenExpirationDate() {
    return localStorage.getItem('token_expiration_date');
  }


  setTokenExpirationDate(expiresIn) {
    const expirationDate = Date.now() + expiresIn * 1000;
    return localStorage.setItem('token_expiration_date', expirationDate.toString());
  }

  isTokenValid() {
    const expirationDate = this.getTokenExpirationDate();
    return parseInt(expirationDate, 10) <= Date.now();
  }
}
