import { Component, OnInit } from '@angular/core';
import { UserService, UserView } from 'src/app/app-services/nswag.generated.services';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { Location } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'app-profile-update',
  templateUrl: './profile-update.component.html',
  styleUrls: ['./profile-update.component.scss']
})
export class ProfileUpdateComponent implements OnInit {
  user: UserView;
  public userInfoUpdateForm: FormGroup;

  constructor(
    private userServuice: UserService,
    private location: Location,
    public router: Router,
    public fb: FormBuilder,
  ) {
    this.userInfoUpdateForm = this.fb.group({
      address: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(90)]],
      phoneNumber: ['', [Validators.required, Validators.minLength(12), Validators.maxLength(13)]],
      name: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(90)]],
      surname: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(90)]]});
   }

  ngOnInit(): void {
    this.userServuice.getProfile().subscribe(data => {this.user = data; },
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

  update(): void {
    this.userServuice.updateUserProfile(this.userInfoUpdateForm.value).subscribe(data => {this.user = data; },
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

  goBack(): void {
    this.location.back();
  }

}
