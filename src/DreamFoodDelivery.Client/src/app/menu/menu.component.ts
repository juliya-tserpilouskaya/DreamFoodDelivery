import { Component, OnInit } from '@angular/core';
import { AuthService } from '../auth/auth.service';

@Component({
  selector: 'app-menu',
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.scss']
})
export class MenuComponent implements OnInit {

  constructor(private authService: AuthService) { }
  // isAuthenticated: boolean;

  ngOnInit(): void {
    // this.isAuthenticated = this.authService.isLoggedIn;
  }
  get isAuthenticated(): boolean {
    return this.authService.isLoggedIn;
  }
}
