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
  spinning = true;
  message: string;

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
        this.message = null;
        this.spinning = false;
        this.isConfirmed = true;
      },
      error => {
        if (error.status ===  400) {
          this.message = 'Error 400: ' + error.response;
        }
        else if (error.status ===  500) {
          this.message = 'Error 500: Internal Server Error!';
        }
        else{
          this.message = 'Something was wrong. Please, contact with us.';
        }
      }

      );
  }

  ngOnInit(): void {
  }
}
