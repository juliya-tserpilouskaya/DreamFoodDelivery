import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, FormControl } from '@angular/forms';

// import service
import { IdentityService } from 'src/app/nswag_gen/services/api.generated.client';
// import models
import { UserRegistration, UserWithToken } from 'src/app/nswag_gen/services/api.generated.client';

import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit {

  registerForm: FormGroup;
  registerRequest: UserRegistration;
  user: UserWithToken;

  constructor(
    private identityService: IdentityService,
    public fb: FormBuilder,
    public router: Router
    ) {
      this.registerForm = this.fb.group({
        name: [''],
        password: ['']
      });
    }

  ngOnInit(): void {
  }

  register() {
    if (this.registerForm.valid) {
      const data = this.registerForm.value;

      this.registerRequest = new UserRegistration();

      this.registerRequest.email = data.name;
      this.registerRequest.password = data.password;

      this.identityService.register(this.registerRequest)
        .subscribe(user => this.user = user);

        this.registerForm.reset();
        this.router.navigate(['login']);
    }
  }


}
