import { Component, OnInit } from '@angular/core';
import { AdminService, UserView, UserDTO } from 'src/app/app-services/nswag.generated.services';
import { Router } from '@angular/router';

@Component({
  selector: 'app-admin-users',
  templateUrl: './admin-users.component.html',
  styleUrls: ['./admin-users.component.scss']
})
export class AdminUsersComponent implements OnInit {
  users: UserView[] = [];
  usersDTO: UserDTO[] = [];
  message: string = null;

  page = 2;
  pageSize = 5;

  constructor(
    private adminService: AdminService,
    public router: Router,
  ) { }

  ngOnInit(): void {
    this.adminService.getAll().subscribe(data => {this.users = data;
                                                  this.usersDTO = this.usersDTO;
                                                },
                                                error => {
                                                  if (error.status ===  206) {
                                                    this.message = error.detail;
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

  removeUser(id: string): void {
    this.adminService.removeById(id).subscribe(data => {
      // const indexToDelete = this.usersDTO.findIndex((mark: UserDTO) => mark.id === id);
      // this.users.splice(indexToDelete, 1);
      this.ngOnInit();
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

  changeRole(idIdentity: string): void {
    this.adminService.changeRole(idIdentity).subscribe(data => {
      this.ngOnInit();
    },
    error => {
      if (error.status ===  403) {
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

  confirmEmail(idIdentity: string): void {
    this.adminService.confirmUserEmail(idIdentity).subscribe(data => {
      this.ngOnInit(); },
      error => {
        if (error.status ===  403) {
          this.message = 'You are not authorized!';
        }
        else if (error.status ===  500) {
          this.message = error.message;
          this.router.navigate(['/error/500', {msg: this.message}]);
        }
        else{
          this.message = 'Something was wrong. Please, contact with us.';
        }
      }
    );
  }

}
