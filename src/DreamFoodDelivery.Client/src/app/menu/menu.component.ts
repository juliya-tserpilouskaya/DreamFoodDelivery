import { Component, OnInit } from '@angular/core';
import { AuthService } from '../auth/auth.service';
import * as jwt_decode from 'jwt-decode';
import { ManageMenuService } from '../app-services/manage-menu.service';

@Component({
  selector: 'app-menu',
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.scss']
})
export class MenuComponent implements OnInit {

  constructor(
    private authService: AuthService,
    private manageMenuService: ManageMenuService,
  ) { }

  ngOnInit(): void {
  }

  get isAuthenticated(): boolean {
    return this.authService.isLoggedIn;
  }

  get isAdmin(): boolean {
    return this.manageMenuService.isAdmin();
  }

  get currentEmail(): string{
    const token = this.authService.getToken();
    const decodedToken = jwt_decode(token);
    return decodedToken.email;
  }
}
