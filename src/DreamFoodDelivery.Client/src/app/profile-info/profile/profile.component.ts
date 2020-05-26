import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/auth/auth.service';
import { UserService, UserView, UserDTO, UserProfile } from 'src/app/app-services/nswag.generated.services';
import { Location } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {
  user: UserView;
  userDTO: UserDTO;
  userProfile: UserProfile;
  isNotFillProfile = false;
  isEmailConfirm = false;
  message: string;

  constructor(
    private authService: AuthService,
    private userService: UserService,
    public router: Router,
    private location: Location,
    ) {}

  ngOnInit(): void {
    this.userService.getProfile().subscribe(data => {this.user = data;
                                                     this.userDTO = data.userDTO;
                                                     this.userProfile = data.userProfile;
                                                     if (this.userProfile.address == null || this.userProfile.email == null
                                                      || this.userProfile.name == null || this.userProfile.surname == null)
                                                     {
                                                      this.isNotFillProfile = true;
                                                      this.message = 'Please, fill your profile. You can do this here, or with order creating.';
                                                     }
                                                     this.isEmailConfirm = this.userProfile.emailConfirmed;
                                                    },
                                                    error => {
                                                      // if (error.status === 500){
                                                      //   this.router.navigate(['/error/500']);
                                                      //  }
                                                      //  else if (error.status === 404) {
                                                      //   this.router.navigate(['/error/404']);
                                                      //  }
                                                      //  else {
                                                      //   this.router.navigate(['/error/unexpected']);
                                                      //  }
   });
  }

  remove(): void {
    this.location.back(); // TODO:
  }


  goBack(): void {
    this.location.back();
  }

  // TODO:  is email confirm

  get isAuthenticated(): boolean {
    return this.authService.isLoggedIn;
  }
}
