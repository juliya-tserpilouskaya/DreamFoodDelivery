import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { UserService } from 'src/app/app-services/nswag.generated.services';
import { Router } from '@angular/router';

@Component({
  selector: 'app-confirm-email',
  templateUrl: './confirm-email.component.html',
  styleUrls: ['./confirm-email.component.scss']
})
export class ConfirmEmailComponent implements OnInit {
  tokenForm: FormGroup;
  isSend = false;

  constructor(
    private userService: UserService,
    public fb: FormBuilder,
    public router: Router
  ) {
    this.tokenForm = fb.group({token: ['']});
   }

  ngOnInit(): void {
  }

  sendToken(): void {
    this.userService.confirmUserEmailSend().subscribe(data => {this.isSend = true;
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

  getToken(): void {
    if (this.tokenForm.valid) {
      this.userService.confirmUserEmailGet(this.tokenForm.value.token)
        .subscribe(user => {this.tokenForm.reset();
                            this.router.navigate(['/profile']);
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
                           }); }
  }

}
