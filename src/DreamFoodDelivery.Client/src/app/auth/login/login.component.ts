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
  message: string = null;

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
    this.message = null;
    this.spinning = true;
    if (this.loginForm.valid) {
      const data = this.loginForm.value;
      this.identityService.login(data)
        .subscribe(user => {this.user = user;
                            this.spinning = false;
                            localStorage.setItem('access_token', this.user.userToken);
                            this.authService.setTokenExpirationDate(this.user.expiresIn);
                            this.currentUser = this.user;
                            this.loginForm.reset();
                            this.router.navigate(['/profile']);
                            this.isAuthenticated =  this.authService.isLoggedIn;
                          },
                          error => {
                            if (error.status ===  400) {
                              this.message = 'Error 400: ' + error.response;
                            }
                            else if (error.status ===  500) {
                              this.message = 'Error 500: Internal Server Error!';
                            }
                            else{
                              this.message = 'Something was wrong. Please, contact with us.';
                            }
                           });
    }
  }
}
