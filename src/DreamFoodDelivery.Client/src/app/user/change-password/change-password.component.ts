import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { UserView, UserService } from 'src/app/app-services/nswag.generated.services';
import { AuthService } from '../../auth/auth.service';
import { Location } from '@angular/common';
import { Router } from '@angular/router';
import * as jwt_decode from 'jwt-decode';

@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.scss']
})
export class ChangePasswordComponent implements OnInit {
  changePassForm: FormGroup;
  message: string = null;
  user: UserView;

  constructor(
    private userService: UserService,
    private authService: AuthService,
    private location: Location,
    public fb: FormBuilder,
    public router: Router
  ) {
    this.changePassForm = fb.group({idFromIdentity: [''],
    currentPassword: [''],
    newPassword: [''],
    });
   }
  ngOnInit(): void {
  }

  changePassword(): void {
    const token = this.authService.getToken();
    const decodedToken = jwt_decode(token);
    this.changePassForm.value.idFromIdentity = decodedToken.id;
    if (this.changePassForm.valid) {
      const data = this.changePassForm.value;
      this.userService.chasngeUserPassword(data)
        .subscribe(user => {this.changePassForm.reset();
                            // this.router.navigate(['/profile']);
                            this.doLogout();
                          },
                          error => {
                            if (error.status ===  206) {
                              this.message = error.detail;
                            }
                            else if (error.status ===  400) {
                              this.message = 'Error 400: ' + error.result400;
                            }
                            else if (error.status ===  403) {
                              this.message = 'You are not authorized!';
                            }
                            else if (error.status ===  500) {
                              this.message = error.message;
                              this.router.navigate(['/error/500', {msg: this.message}]);
                            }
                            else{
                              this.message = 'Something was wrong. Please, contact with us.';
                            }
                           }); }
  }

  doLogout() {
    const removeToken = localStorage.removeItem('access_token');
    if (removeToken == null) {
      this.router.navigate(['/menu']);
    }
  }

  goBack(): void {
    this.location.back();
  }

}
