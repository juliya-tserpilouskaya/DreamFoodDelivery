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

  constructor(
    private orderService: OrderService,
    private userService: UserService,
    private location: Location,
    private router: Router,
    public fb: FormBuilder,
  ) {this.orderAddForm = this.fb.group({
      isInfoFromProfile: [false],
      address: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(90)]],
      phoneNumber: ['', [Validators.required, Validators.minLength(12), Validators.maxLength(13)]],
      name: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(90)]],
      surname: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(90)]]});
 }

  ngOnInit(): void {
    this.userService.getProfile().subscribe(data => {this.user = data; },
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
    this.orderAddForm.setValue({
      isInfoFromProfile: false,
      address: this.user.userProfile.address,
      phoneNumber: this.user.userProfile.phoneNumber,
      name: this.user.userProfile.name,
      surname: this.user.userProfile.surname,
    });
  }

  goBack(): void {
    this.location.back();
  }
}
