import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { IdentityService } from 'src/app/app-services/nswag.generated.services';
import { Router } from '@angular/router';

@Component({
  selector: 'app-delete-account',
  templateUrl: './delete-account.component.html',
  styleUrls: ['./delete-account.component.scss']
})
export class DeleteAccountComponent implements OnInit {
  deleteForm: FormGroup;

  constructor(
    private identityService: IdentityService,
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
  }

  doLogout() {
    const removeToken = localStorage.removeItem('access_token');
    if (removeToken == null) {
      this.router.navigate(['/menu']);
    }
  }

}
