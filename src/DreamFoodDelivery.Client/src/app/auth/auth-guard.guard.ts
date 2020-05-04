import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';

import { LoginComponent } from './login/login.component';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})

export class AuthGuardGuard implements CanActivate {

  constructor(
    private authComponent: LoginComponent,
    public router: Router
  ) { }

  canActivate(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
      if (this.authComponent.isLoggedIn !== true) {
        window.alert("Access not allowed!");
        this.router.navigate(['login'])
      }
  
    return true;
  }
  
}
