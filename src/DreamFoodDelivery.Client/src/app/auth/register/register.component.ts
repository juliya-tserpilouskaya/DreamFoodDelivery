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
        password: ['']
      });
    }
    isAuthenticated: boolean;

  ngOnInit(): void {
  }

  register() {
    if (this.registerForm.valid) {
      const data = this.registerForm.value;
      this.identityService.register(data)
        .subscribe(user => {this.user = user;
                            localStorage.setItem('access_token', this.user.userToken);
                            this.currentUser = this.user;
                            this.registerForm.reset();
                            this.router.navigate(['/profile']);
                            this.isAuthenticated =  this.authService.isLoggedIn;
                          },
                          error => {
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
