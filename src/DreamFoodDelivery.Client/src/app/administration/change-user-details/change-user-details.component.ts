import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AdminService, UserView } from 'src/app/app-services/nswag.generated.services';

@Component({
  selector: 'app-change-user-details',
  templateUrl: './change-user-details.component.html',
  styleUrls: ['./change-user-details.component.scss']
})
export class ChangeUserDetailsComponent implements OnInit {
  id = '';
  user: UserView;


  constructor(
    route: ActivatedRoute,
    private adminService: AdminService
    ) {
    // tslint:disable-next-line: no-string-literal
    route.params.subscribe(params => this.id = params['id']);
   }

  ngOnInit(): void {
    this.adminService.getById(this.id).subscribe(data => this.user = data);
  }
// тут должно происходить обновление данных, установка скидки, обвновление данных профиля
}
