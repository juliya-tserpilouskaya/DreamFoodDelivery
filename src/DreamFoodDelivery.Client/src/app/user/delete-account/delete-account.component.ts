import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { IdentityService } from 'src/app/app-services/nswag.generated.services';
import { Location } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'app-delete-account',
  templateUrl: './delete-account.component.html',
  styleUrls: ['./delete-account.component.scss']
})
export class DeleteAccountComponent implements OnInit {
  deleteForm: FormGroup;
  message: string = null;

  constructor(
    private identityService: IdentityService,
    private location: Location,
    public fb: FormBuilder,
    public router: Router) {
      this.deleteForm = fb.group({
        email: [''],
        password: ['']
      });
    }

  ngOnInit(): void {
  }

  onDelete() {
    if (this.deleteForm.valid) {
      const data = this.deleteForm.value;
      this.identityService.delete(data)
        .subscribe(user => {this.doLogout();
        },
        error => {
          if (error.status ===  206) {
            this.message = error.detail;
          }
          else if (error.status ===  400) {
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
  }

  doLogout() {
    const removeToken = localStorage.removeItem('access_token');
    if (removeToken == null) {
      this.router.navigate(['/menu']);
    }
  }

  goBack(): void {
    this.location.back();
  }

}
