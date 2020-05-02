import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';

// import service
import { IdentityService } from 'src/app/nswag_gen/services/api.generated.client';
// import models
import { UserRegistration, UserWithToken } from 'src/app/nswag_gen/services/api.generated.client';

import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {

  loginForm: FormGroup;
  loginRequest: UserRegistration;
  user: UserWithToken;
  currentUser = {};

  constructor(
    private identityService: IdentityService,
    fb: FormBuilder,
    public router: Router) {
      this.loginForm = fb.group({
        name: [''],
        password: ['']
      });
    }

  ngOnInit(): void {
  }

  login() {
    if (this.loginForm.valid) {
      const data = this.loginForm.value;

      this.loginRequest = new UserRegistration();

      this.loginRequest.email = data.name;
      this.loginRequest.password = data.password;

      this.identityService.login(this.loginRequest)
        .subscribe(user => this.user = user);
          localStorage.setItem('access_token', this.user.userToken)
          this.currentUser = this.user;

          this.router.navigate(['profile']);
          this.loginForm.reset();

    }
  }

  getToken() {
    return localStorage.getItem('access_token');
  }

  get isLoggedIn(): boolean {
    let authToken = localStorage.getItem('access_token');
    return (authToken !== null) ? true : false;
  }

  doLogout() {
    let removeToken = localStorage.removeItem('access_token');
    if (removeToken == null) {
      this.router.navigate(['login']);
    }
  }
}
