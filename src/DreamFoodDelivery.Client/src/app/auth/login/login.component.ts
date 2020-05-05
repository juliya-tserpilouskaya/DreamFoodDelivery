import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';

// import service and models
import { UserWithToken, IdentityService } from 'src/app/app-services/nswag.generated.services';

import { Router } from '@angular/router';
import { AuthService } from '../auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {

  loginForm: FormGroup;
  user: UserWithToken;
  currentUser = {};

  constructor(
    private identityService: IdentityService,
    private authService: AuthService,
    public fb: FormBuilder,
    public router: Router) {
      this.loginForm = fb.group({
        email: [''],
        password: ['']
      });
    }
    isAuthenticated: boolean;

  ngOnInit(): void {
  }

  login() {
    if (this.loginForm.valid) {
      const data = this.loginForm.value;
      this.identityService.login(data)
        .subscribe(user => {this.user = user;
                            localStorage.setItem('access_token', this.user.userToken);
                            this.currentUser = this.user;
                            this.loginForm.reset();
                            this.router.navigate(['/profile']);
                            this.isAuthenticated =  this.authService.isLoggedIn;
                           });
    }
  }
}
