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
  spinning = false;
  errorMessage: string;

  loginForm: FormGroup;
  user: UserWithToken;
  isAuthenticated: boolean;
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

  ngOnInit(): void {
  }

  login() {
    this.errorMessage = null;
    this.spinning = true;
    if (this.loginForm.valid) {
      const data = this.loginForm.value;
      this.identityService.login(data)
        .subscribe(user => {this.user = user;
                            this.spinning = false;
                            localStorage.setItem('access_token', this.user.userToken);
                            this.currentUser = this.user;
                            this.loginForm.reset();
                            this.router.navigate(['/profile']);
                            this.isAuthenticated =  this.authService.isLoggedIn;
                          },
                          error => {
                            this.spinning = false;
                            if (error.status === 500){
                              this.router.navigate(['/error/500']);
                             }
                             else if (error.status === 404) {
                              this.router.navigate(['/error/404']);
                             }
                            //  else {
                            //   this.router.navigate(['/error/unexpected']);
                            //  }
                           });
    }
  }
}
