import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { UserService } from 'src/app/app-services/nswag.generated.services';

@Component({
  selector: 'app-password-reset',
  templateUrl: './password-reset.component.html',
  styleUrls: ['./password-reset.component.scss']
})
export class PasswordResetComponent implements OnInit {
  spinning = false;
  errorMessage: string;

  userId: string;
  token: string;

  resetForm: FormGroup;

  isReseted = false;

  constructor(
    private route: ActivatedRoute,
    public fb: FormBuilder,
    public router: Router,
    private userService: UserService
  ) {
    this.userId = route.snapshot.queryParamMap.get('userId');
    this.token = route.snapshot.queryParamMap.get('token');

    this.resetForm = this.fb.group({
      userId: this.userId,
      token: this.token,
      password: '',
      callBackUrl: 'http://localhost:4200/login'
    });
  }

  ngOnInit(): void {
  }

  reset(){
    this.errorMessage = null;
    this.spinning = true;

    if (this.resetForm.valid) {
      this.userService.resetPassword(this.resetForm.value)
      .subscribe(() =>
      {
        this.spinning = false;
        this.isReseted = true;
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
