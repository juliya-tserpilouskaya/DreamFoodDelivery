import { Component, OnInit } from '@angular/core';
import { AuthService } from '../auth/auth.service';
import * as jwt_decode from 'jwt-decode';

@Component({
  selector: 'app-menu',
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.scss']
})
export class MenuComponent implements OnInit {

  constructor(private authService: AuthService) { }

  ngOnInit(): void {
  }

  get isAuthenticated(): boolean {
    return this.authService.isLoggedIn;
  }

  get isAdmin(): boolean {
    const token = this.authService.getToken();
    const decodedToken = jwt_decode(token);
    // tslint:disable-next-line: no-string-literal
    const currentRole = decodedToken['role'];
    if (currentRole.includes('Admin')) {
      return true;
    }
    return false;
  }

  get currentEmail(): string{
    const token = this.authService.getToken();
    const decodedToken = jwt_decode(token);
    return decodedToken.email;
  }
}
