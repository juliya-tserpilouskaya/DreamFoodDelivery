import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { UserView, UserService } from 'src/app/app-services/nswag.generated.services';
import { AuthService } from '../auth.service';
import { Router } from '@angular/router';
import * as jwt_decode from 'jwt-decode';

@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.scss']
})
export class ChangePasswordComponent implements OnInit {
  changePassForm: FormGroup;
  user: UserView;

  constructor(
    private userService: UserService,
    private authService: AuthService,
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
    const decodedoken = jwt_decode(token);
    this.changePassForm.value.idFromIdentity = decodedoken.id;
    if (this.changePassForm.valid) {
      const data = this.changePassForm.value;
      this.userService.chasngeUserPassword(data)
        .subscribe(user => {this.changePassForm.reset();
                            this.router.navigate(['/profile']);
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
                           }); }
  }

}
