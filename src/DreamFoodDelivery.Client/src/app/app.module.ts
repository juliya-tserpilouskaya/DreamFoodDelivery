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
         TagService } from './app-services/nswag.generated.services';

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
    TagService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
