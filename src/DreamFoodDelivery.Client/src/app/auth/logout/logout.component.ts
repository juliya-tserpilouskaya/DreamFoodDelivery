import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { IdentityService } from 'src/app/app-services/nswag.generated.services';

@Component({
  selector: 'app-logout',
  templateUrl: './logout.component.html',
  styleUrls: ['./logout.component.scss']
})
export class LogoutComponent implements OnInit {

  constructor(public router: Router, private identityService: IdentityService) { }

  ngOnInit(): void {
  }

  doLogout() {
    const removeToken = localStorage.removeItem('access_token');
    if (removeToken == null) {
      // this.identityService.logOut().subscribe();
      this.router.navigate(['/menu']);
    }
  }
}
