import { Component, OnInit } from '@angular/core';
import { Location } from '@angular/common';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { OrderService, UserService, UserView, OrderView } from 'src/app/app-services/nswag.generated.services';

@Component({
  selector: 'app-order-create',
  templateUrl: './order-create.component.html',
  styleUrls: ['./order-create.component.scss']
})
export class OrderCreateComponent implements OnInit {
  orderAddForm: FormGroup;
  user: UserView;
  order: OrderView;
  isNotConfirmed = false;
  message: string = null;

  constructor(
    private orderService: OrderService,
    private userService: UserService,
    private location: Location,
    private router: Router,
    public fb: FormBuilder,
  ) {this.orderAddForm = this.fb.group({
      isInfoFromProfile: [false],
      updateProfile: [false],
      address: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(90)]],
      phoneNumber: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(13)]],
      name: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(90)]],
      surname: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(90)]]});

 }

  ngOnInit(): void {
    this.userService.getProfile().subscribe(data => {this.user = data; },
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

  addNewOrder(): void {
    if (this.orderAddForm.valid) {
      if (this.orderAddForm.value.address === this.user.userProfile.address &&
        this.orderAddForm.value.phoneNumber === this.user.userProfile.phoneNumber &&
        this.orderAddForm.value.name === this.user.userProfile.name &&
        this.orderAddForm.value.surname === this.user.userProfile.surname)
        {
        this.orderAddForm.value.isInfoFromProfile = true;
      }
      console.log(this.orderAddForm.value);
      this.orderService.create(this.orderAddForm.value)
        .subscribe(data => { this.order = data;
                             this.router.navigate(['/order', this.order.id, 'details']);
                            },
                            error => {
                            if (error.status ===  400) {
                              this.message = 'Error 400: ' + error.response;
                            }
                            else if (error.status ===  401) {
                              this.message = 'You are not authorized!';
                            }
                            else if (error.status ===  403) {
                              this.message = 'You are not authorized!';
                            }
                            else if (error.status ===  500) {
                              this.message = 'Error 500: Internal Server Error!';
                            }
                            else{
                              this.message = 'Something was wrong. Please, contact with us.';
                            }
       });
      this.userService.updateUserProfile(this.orderAddForm.value).subscribe();
    }
  }

  getdataFromProfile(): void {
    this.orderAddForm.setValue({
      isInfoFromProfile: false,
      updateProfile: false,
      address: this.user.userProfile.address,
      phoneNumber: this.user.userProfile.phoneNumber,
      name: this.user.userProfile.name,
      surname: this.user.userProfile.surname,
      // TODO: if null - message
    });
  }

  goBack(): void {
    this.location.back();
  }
}
