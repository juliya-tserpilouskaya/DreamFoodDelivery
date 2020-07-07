import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {

  constructor() { }
}

// creatNotification(message: string): string {
//   if (error.status ===  400) {
//     this.message = 'Error 400: ' + error.response;
//   }
//   else if (error.status ===  500) {
//     this.message = 'Error 500: Internal Server Error!';
//   }
//   else{
//     this.message = 'Something was wrong. Please, contact with us.';
//   }

// }
