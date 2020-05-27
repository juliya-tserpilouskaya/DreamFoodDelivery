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
  message: string = null;
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
      phoneNumber: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(13)]],
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
          const decodedToken = jwt_decode(token);
          const currentId = decodedToken.id;
          if (currentId === this.user.userDTO.idFromIdentity){
          this.isOwner = true;
          console.log(true);
        }
        });
      },
      error => {
        if (error.status ===  204) {
          this.message = 'Now empty.';
        }
        else if (error.status ===  400) {
          this.message = 'Error 400: ' + error.response;
        }
        else if (error.status ===  403) {
          this.message = 'You are not authorized!';
        }
        else if (error.status ===  404) {
          this.message = 'Element not found.';
        }
        else if (error.status ===  500) {
          this.message = 'Error 500: Internal Server Error!';
        }
        else{
          this.message = 'Something was wrong. Please, contact with us.';
        }
    });
  }

  updateOrder(id: string): void {
    this.orderInfoUpdateForm.value.id = id;
    if (this.orderInfoUpdateForm.valid) {
      if (this.orderInfoUpdateForm.value.address === this.user.userProfile.address &&
        this.orderInfoUpdateForm.value.phoneNumber === this.user.userProfile.phoneNumber &&
        this.orderInfoUpdateForm.value.name === this.user.userProfile.name &&
        this.orderInfoUpdateForm.value.surname === this.user.userProfile.surname)
        {
        this.orderInfoUpdateForm.value.isInfoFromProfile = true;
        }
      console.log(this.orderInfoUpdateForm.value);

      this.orderService.update(this.orderInfoUpdateForm.value).subscribe(data => {this.order = data;
                                                                                  this.done = true;
                                                                                },
                                                                                error => {
                                                                                  if (error.status ===  400) {
                                                                                    this.message = 'Error 400: ' + error.response;
                                                                                  }
                                                                                  else if (error.status ===  403) {
                                                                                    this.message = 'You are not authorized!';
                                                                                  }
                                                                                  else if (error.status ===  404) {
                                                                                    this.message = 'Element not found.';
                                                                                  }
                                                                                  else if (error.status ===  500) {
                                                                                    this.message = 'Error 500: Internal Server Error!';
                                                                                  }
                                                                                  else{
                                                                                    this.message = 'Something was wrong. Please, contact with us.';
                                                                                  }
      });
    }
  }

  getdataFromProfile(): void {
    if (this.isOwner) {
      this.orderInfoUpdateForm.setValue({
        isInfoFromProfile: false,
        address: this.user.userProfile.address,
        phoneNumber: this.user.userProfile.phoneNumber,
        name: this.user.userProfile.name,
        surname: this.user.userProfile.surname,
      });
    }
  }

  removeById(id: string): void {
    this.orderService.removeById(id).subscribe(data => { this.router.navigate(['/administration/orders']);
    },
    error => {
      if (error.status ===  400) {
        this.message = 'Error 400: ' + error.response;
      }
      else if (error.status ===  403) {
        this.message = 'You are not authorized!';
      }
      else if (error.status ===  404) {
        this.message = 'Element not found.';
      }
      else if (error.status ===  500) {
        this.message = 'Error 500: Internal Server Error!';
      }
      else{
        this.message = 'Something was wrong. Please, contact with us.';
      }
       });
  }

  goBack(): void {
    this.location.back();
  }

  get isAdmin(): boolean {
    const token = this.authService.getToken();
    const decodedToken = jwt_decode(token);
    // tslint:disable-next-line: no-string-literal
    const currentRole = decodedToken['role'];
    if (currentRole.includes('Admin')) {
      return true;
    }
    return false;
  }
}
