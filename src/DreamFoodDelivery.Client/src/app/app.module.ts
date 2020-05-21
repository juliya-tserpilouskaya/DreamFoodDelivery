import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';

import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { IdentityService,
         UserService,
         MenuService,
         DishService,
         BasketService,
         OrderService,
         CommentService,
         TagService,
         AdminService,
         SearchService} from './app-services/nswag.generated.services';

import { AppComponent } from './app.component';
import { MenuComponent } from './menu/menu.component';

import { LoginComponent } from './auth/login/login.component';
import { RegisterComponent } from './auth/register/register.component';

import { PageNotFoundComponent } from './pages/page-not-found/page-not-found.component';
import { BasketComponent } from './basket/basket.component';
import { LogoutComponent } from './auth/logout/logout.component';
import { AdministrationComponent } from './administration/administration.component';
import { MenuSearchComponent } from './menu-search/menu-search.component';
import { AboutUsComponent } from './pages/about-us/about-us.component';
import { ProfileComponent } from './profile-info/profile/profile.component';
import { ProfileUpdateComponent } from './profile-info/profile-update/profile-update.component';
import { HomeComponent } from './pages/home/home.component';
import { AuthGuard } from './auth/auth.guard';
import { AuthService } from './auth/auth.service';
import { TokenInterceptorService } from './auth/token.interceptor.service';
import {NgbModule} from '@ng-bootstrap/ng-bootstrap';
import { BadRequestComponent } from './pages/bad-request/bad-request.component';
import { InternalServerErrorComponent } from './pages/internal-server-error/internal-server-error.component';

import { OrderCreateComponent } from './orders/order-create/order-create.component';
import { OrderDetailsComponent } from './orders/order-details/order-details.component';
import { UserOrdersComponent } from './orders/user-orders/user-orders.component';

import { UserCommentsComponent } from './comments/user-comments/user-comments.component';
import { CommentCreateComponent } from './comments/comment-create/comment-create.component';
import { ChangeUserDetailsComponent } from './administration/change-user-details/change-user-details.component';
import { AdminCommentsComponent } from './administration/admin-comments/admin-comments.component';
import { AdminsOrdersComponent } from './administration/admins-orders/admins-orders.component';
import { DishesComponent } from './dishes/dishes.component';
import { AdminUsersComponent } from './administration/admin-users/admin-users.component';
import { AdminNavBarComponent } from './administration/admin-nav-bar/admin-nav-bar.component';
import { CommentUpdateComponent } from './comments/comment-update/comment-update.component';
import { OrderUpdateComponent } from './orders/order-update/order-update.component';
import { EmployeeComponent } from './employee/employee.component';
import { EmployeeNavBarComponent } from './employee/employee-nav-bar/employee-nav-bar.component';
import { EmployeeOrdersComponent } from './employee/employee-orders/employee-orders.component';
import { DishUpdateComponent } from './dishes/dish-update/dish-update.component';
import { DishAddNewComponent } from './dishes/dish-add-new/dish-add-new.component';
import { UserOrdersAdminComponent } from './orders/user-orders-admin/user-orders-admin.component';
import { UserCommentsAdminComponent } from './comments/user-comments-admin/user-comments-admin.component';
import { DeleteAccountComponent } from './auth/delete-account/delete-account.component';
import { ChangeEmailComponent } from './auth/change-email/change-email.component';
import { ChangePasswordComponent } from './auth/change-password/change-password.component';
import { ConfirmEmailComponent } from './auth/confirm-email/confirm-email.component';
import { UnexpectedComponent } from './pages/unexpected/unexpected.component';
import { ImageUploadComponent } from './images/image-upload/image-upload.component';
import { ImagesGetComponent } from './images/images-get/images-get.component';
import { ImageModifiedService } from './app-services/image.services';



@NgModule({
  declarations: [
    AppComponent,
    MenuComponent,
    RegisterComponent,
    LoginComponent,
    LogoutComponent,
    ProfileComponent,
    ProfileUpdateComponent,
    MenuSearchComponent,
    BasketComponent,
    AdministrationComponent,
    HomeComponent,
    AboutUsComponent,
    PageNotFoundComponent,
    BadRequestComponent,
    InternalServerErrorComponent,
    OrderCreateComponent,
    UserOrdersComponent,
    OrderDetailsComponent,
    UserCommentsComponent,
    CommentCreateComponent,
    ChangeUserDetailsComponent,
    AdminCommentsComponent,
    AdminsOrdersComponent,
    DishesComponent,
    AdminUsersComponent,
    AdminNavBarComponent,
    CommentUpdateComponent,
    OrderUpdateComponent,
    EmployeeComponent,
    EmployeeNavBarComponent,
    EmployeeOrdersComponent,
    DishUpdateComponent,
    DishAddNewComponent,
    UserOrdersAdminComponent,
    UserCommentsAdminComponent,
    DeleteAccountComponent,
    ChangeEmailComponent,
    ChangePasswordComponent,
    ConfirmEmailComponent,
    UnexpectedComponent,
    ImageUploadComponent,
    ImagesGetComponent,
  ],
  imports: [
    BrowserAnimationsModule,
    BsDatepickerModule.forRoot(),
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    NgbModule
  ],
  providers: [
    AuthGuard,
    AuthService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: TokenInterceptorService,
      multi: true
    },
    IdentityService,
    UserService,
    MenuService,
    DishService,
    BasketService,
    OrderService,
    CommentService,
    TagService,
    AdminService,
    SearchService,
    ImageModifiedService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
