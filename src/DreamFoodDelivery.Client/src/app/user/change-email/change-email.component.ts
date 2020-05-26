import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { UserService, UserView } from 'src/app/app-services/nswag.generated.services';
import { Router } from '@angular/router';
import { Location } from '@angular/common';
import { AuthService } from '../../auth/auth.service';
import * as jwt_decode from 'jwt-decode';

@Component({
  selector: 'app-change-email',
  templateUrl: './change-email.component.html',
  styleUrls: ['./change-email.component.scss']
})
export class ChangeEmailComponent implements OnInit {
  changeEmailForm: FormGroup;
  user: UserView;

  constructor(
    private userService: UserService,
    private authService: AuthService,
    public fb: FormBuilder,
    private location: Location,
    public router: Router
  ) {
    this.changeEmailForm = fb.group({idFromIdentity: [''],
    newEmail: ['']
  }); }

  ngOnInit(): void {
  }

  change(): void {
    const token = this.authService.getToken();
    const decodedToken = jwt_decode(token);
    this.changeEmailForm.value.idFromIdentity = decodedToken.id;
    if (this.changeEmailForm.valid) {
      const data = this.changeEmailForm.value;
      this.userService.changeUserEmail(data)
        .subscribe(user => {this.user = user;
                            this.changeEmailForm.reset();
                            // this.router.navigate(['/profile']);
                            this.doLogout();
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
