import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AdminService, UserView } from 'src/app/app-services/nswag.generated.services';
import { Location } from '@angular/common';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';

@Component({
  selector: 'app-change-user-details',
  templateUrl: './change-user-details.component.html',
  styleUrls: ['./change-user-details.component.scss']
})
export class ChangeUserDetailsComponent implements OnInit {
  idFromURL = '';
  user: UserView;
  done = false;

  userInfoUpdateForm: FormGroup;
  userPersonalDiscountForm: FormGroup;

  constructor(
    private route: ActivatedRoute,
    private adminService: AdminService,
    private location: Location,
    public router: Router,
    public fb: FormBuilder,
    ) {
    route.params.subscribe(params => this.idFromURL = params.id);
    this.userInfoUpdateForm = fb.group({address: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(90)]],
                                        phoneNumber: ['', [Validators.required, Validators.minLength(12), Validators.maxLength(13)]],
                                        name: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(90)]],
                                        surname: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(90)]]});
    this.userPersonalDiscountForm = fb.group({personalDiscount: ['', [Validators.required, Validators.min(0), Validators.max(100)]]});
   }

  ngOnInit(): void {
    this.adminService.getById(this.idFromURL).subscribe(data => {this.user = data;
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
  getUserinfo(): void {
    const idUrl = this.route.snapshot.paramMap.get('id');
    this.adminService.getById(idUrl).subscribe(data => {this.user = data;
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
  updateUserInfo(id: string): void {
    this.adminService.updateUserProfileByAdmin(id, this.userInfoUpdateForm.value)
    .subscribe(data => {this.user = data;
                        this.done = true;
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
  updatePersonalDiscount(id: string): void {
    this.adminService.updatePersonalDiscount(id, this.userPersonalDiscountForm.value.personalDiscount)
    .subscribe(data => {this.user = data;
                        this.done = true;
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
  goBack(): void {
    this.location.back();
  }
}
