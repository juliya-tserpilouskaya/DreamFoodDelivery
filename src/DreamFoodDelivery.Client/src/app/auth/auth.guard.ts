import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { Observable } from 'rxjs';

import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})

export class AuthGuard implements CanActivate {

  constructor(
    public router: Router,
    public authService: AuthService
  ) { }

  canActivate(): boolean {
    if (this.authService.isLoggedIn) {
      return true;
    }
    else{
      window.alert('Access not allowed!');
      console.log('Access not allowed!');
      this.router.navigate(['/login']);
    }
      // if (!this.authService.isLoggedIn) {
      //   window.alert('Access not allowed!');
      //   this.router.navigate(['/login']);
      // }
      // else{
      //   return true;
      // }
  }
}
