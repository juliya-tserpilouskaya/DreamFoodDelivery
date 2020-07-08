import { Component, OnInit } from '@angular/core';
import { UserService, UserView } from 'src/app/app-services/nswag.generated.services';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { Location } from '@angular/common';
import { Router } from '@angular/router';
import { ImageModifiedService } from 'src/app/app-services/image.services';

@Component({
  selector: 'app-profile-update',
  templateUrl: './profile-update.component.html',
  styleUrls: ['./profile-update.component.scss']
})
export class ProfileUpdateComponent implements OnInit {
  user: UserView;
  message: string = null;
  public userInfoUpdateForm: FormGroup;

  constructor(
    private userServuice: UserService,
    private imageService: ImageModifiedService,
    private location: Location,
    public router: Router,
    public fb: FormBuilder,
  ) {
    this.userInfoUpdateForm = this.fb.group({
      address: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(90)]],
      phoneNumber: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(13)]],
      name: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(90)]],
      surname: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(90)]]});
   }

  ngOnInit(): void {
    this.userServuice.getProfile().subscribe(data => {this.user = data; },
      error => {
        if (error.status ===  206) {
          this.message = error.detail;
        }
        else if (error.status ===  400) {
          this.message = 'Error 400: ' + error.response;
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
     });
  }

  update(): void {
    this.userServuice.updateUserProfile(this.userInfoUpdateForm.value).subscribe(data => {this.user = data; },
      error => {
        if (error.status ===  400) {
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
      });
  }

  goBack(): void {
    this.location.back();
  }

}
