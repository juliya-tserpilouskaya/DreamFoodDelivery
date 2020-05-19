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

  get isAuthenticated(): boolean {
    return this.authService.isLoggedIn;
  }
}
