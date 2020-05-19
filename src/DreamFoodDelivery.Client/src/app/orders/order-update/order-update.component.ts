import { Component, OnInit } from '@angular/core';
import { Location } from '@angular/common';
import { OrderView, OrderService, UserView, UserService } from 'src/app/app-services/nswag.generated.services';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from 'src/app/auth/auth.service';
import * as jwt_decode from 'jwt-decode';

@Component({
  selector: 'app-order-update',
  templateUrl: './order-update.component.html',
  styleUrls: ['./order-update.component.scss']
})
export class OrderUpdateComponent implements OnInit {
  idFromURL = '';
  order: OrderView;
  user: UserView;
  done = false;
  isOwner = false;
  public orderInfoUpdateForm: FormGroup;

  constructor(
    private route: ActivatedRoute,
    private orderService: OrderService,
    private userService: UserService,
    private authService: AuthService,
    public router: Router,
    public fb: FormBuilder,
    private location: Location,
  ) {
    route.params.subscribe(params => this.idFromURL = params.id);
    this.orderInfoUpdateForm = this.fb.group({
      isInfoFromProfile: [false, [Validators.required]],
      address: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(90)]],
      phoneNumber: ['', [Validators.required, Validators.minLength(12), Validators.maxLength(13)]],
      name: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(90)]],
      surname: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(90)]]});
   }

  ngOnInit(): void {
    this.orderService.getById(this.idFromURL).subscribe(data =>
      {
        this.order = data;
        this.userService.getProfile().subscribe(profile => {
          this.user = profile;
          const token = this.authService.getToken();
          const decodedoken = jwt_decode(token);
          const currentId = decodedoken.id;
          if (currentId === this.user.userDTO.idFromIdentity){
          this.isOwner = true;
          // console.log(true);
        }
        });
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

  updateOrder(id: string): void {
    this.orderInfoUpdateForm.value.id = id;
    if (this.orderInfoUpdateForm.valid) {
      this.orderService.update(this.orderInfoUpdateForm.value).subscribe(data => {this.order = data;
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
    } else {
      // TODO: message
    }
  }

  getdataFromProfile(): void {
    if (!this.isAdmin) {
      this.orderInfoUpdateForm.value.address = this.user.userProfile.address;
      this.orderInfoUpdateForm.value.phoneNumber = this.user.userProfile.phoneNumber;
      this.orderInfoUpdateForm.value.name = this.user.userProfile.name;
      this.orderInfoUpdateForm.value.surname = this.user.userProfile.surname;
      if (this.orderInfoUpdateForm.value.valid) {
        // console.log('valid');
        this.orderInfoUpdateForm.value.isInfoFromProfile = true;
        // console.log(this.orderInfoUpdateForm.value.valid);
      }
      else {
        console.log('not valid');
        // TODO: message
      }
    }
  }

  removeById(id: string): void {
    this.orderService.removeById(id).subscribe(data => {
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

  get isAdmin(): boolean {
    const token = this.authService.getToken();
    const decodedoken = jwt_decode(token);
    // tslint:disable-next-line: no-string-literal
    const currentRole = decodedoken['role'];
    if (currentRole.includes('Admin')) {
      return true;
    }
    return false;
  }
}
