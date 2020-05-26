import { Component, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { UserService } from 'src/app/app-services/nswag.generated.services';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-email-confirm-by-link',
  templateUrl: './email-confirm-by-link.component.html',
  styleUrls: ['./email-confirm-by-link.component.scss']
})
export class EmailConfirmByLinkComponent implements OnInit {
  userId: string;
  token: string;

  isConfirmed = false;

  private querySubscription: Subscription;

  constructor(
    private route: ActivatedRoute,
    private userService: UserService,
  ) {
      this.userId = route.snapshot.queryParamMap.get('userId');
      this.token = route.snapshot.queryParamMap.get('token');

      this.userService.confirmEmail(this.userId, this.token)
      .subscribe(() => {
        this.isConfirmed = true;
      });
  }

  ngOnInit(): void {
  }
}
