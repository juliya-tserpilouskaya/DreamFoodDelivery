import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, FormControl } from '@angular/forms';

// import service and models
import { UserWithToken, IdentityService } from 'src/app/app-services/nswag.generated.services';

import { Router } from '@angular/router';
import { AuthService } from '../auth.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit {

  spinning = false;
  message: string = null;

  registerForm: FormGroup;
  user: UserWithToken;
  currentUser = {};

  constructor(
    private identityService: IdentityService,
    private authService: AuthService,
    public fb: FormBuilder,
    public router: Router
    ) {
      this.registerForm = this.fb.group({
        email: [''],
        password: [''],
        callBackUrl: 'http://localhost:4200/confirmation'
      });
    }
    isAuthenticated: boolean;

  ngOnInit(): void {
  }

  register() {
    this.message = null;
    this.spinning = true;
    if (this.registerForm.valid) {
      const data = this.registerForm.value;
      this.identityService.register(data)
        .subscribe(user => {this.user = user;
                            this.spinning = false;
                            localStorage.setItem('access_token', this.user.userToken);
                            this.authService.setTokenExpirationDate(this.user.expiresIn);
                            this.currentUser = this.user;
                            this.registerForm.reset();
                            this.router.navigate(['/profile']);
                            this.isAuthenticated =  this.authService.isLoggedIn;
                          },
                          error => {
                            this.spinning = false;
                            if (error.status ===  206) {
                              this.message = error.detail;
                            }
                            else if (error.status ===  400) {
                              this.message = 'Error 400: ' + error.result400;
                            }
                            else if (error.status ===  500) {
                              this.message = error.message;
                              this.router.navigate(['/error/500', {msg: this.message}]);
                            }
                            else{
                              this.message = 'Something was wrong. Please, contact with us.';
                            }
                           });
   }
  }
}
