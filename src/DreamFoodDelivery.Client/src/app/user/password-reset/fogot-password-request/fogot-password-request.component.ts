import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { UserService } from 'src/app/app-services/nswag.generated.services';
import { Router } from '@angular/router';

@Component({
  selector: 'app-fogot-password-request',
  templateUrl: './fogot-password-request.component.html',
  styleUrls: ['./fogot-password-request.component.scss']
})
export class FogotPasswordRequestComponent implements OnInit {
  spinning = false;
  errorMessage: string;

  forgotForm: FormGroup;

  isOk = false;

  constructor(
    private userService: UserService,
    public fb: FormBuilder,
    public router: Router
  ) {
    this.forgotForm = this.fb.group({
      callBackUrl: 'http://localhost:4200/password/reset',
      Email: ''
    });
  }

  ngOnInit(): void {
  }

  sendForgotMessage(){
    this.errorMessage = null;
    this.spinning = true;

    if (this.forgotForm.valid) {
      this.userService.forgotPassword(this.forgotForm.value)
      .subscribe(() =>
      {
        this.spinning = false;
        this.isOk = true;
      },
      error => {
        this.spinning = false;
        if (error.status ===  206) {
          this.errorMessage = error.detail;
        }
        else if (error.status ===  400) {
          this.errorMessage = 'Error 400: ' + error.result400;
        }
        else if (error.status ===  500) {
          this.errorMessage = error.message;
          this.router.navigate(['/error/500', {msg: this.errorMessage}]);
        }
        else{
          this.errorMessage = 'Something was wrong. Please, contact with us.';
        }
      });
    }
    else
    {
      this.spinning = false;
      this.errorMessage = 'Data entered incorrectly.';
    }
  }

}
