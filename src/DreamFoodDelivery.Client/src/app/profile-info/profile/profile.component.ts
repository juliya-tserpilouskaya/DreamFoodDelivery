import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/auth/auth.service';
import { UserService, UserView, UserDTO, UserProfile } from 'src/app/app-services/nswag.generated.services';

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
    ) {}

  ngOnInit(): void {
    this.userService.getProfile().subscribe(data => {this.user = data;
                                                     this.userDTO = data.userDTO;
                                                     this.userProfile = data.userProfile;
   });
  }

  get isAuthenticated(): boolean {
    return this.authService.isLoggedIn;
  }
}
