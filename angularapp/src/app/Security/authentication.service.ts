import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';

import { ApplicationUser } from './Models/application-user.model';
import { NotificationsService } from '../Notifications/Notifications.service';
import { NotificationType } from '../Notifications/NotificationType';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {
  ApplicationUser: ApplicationUser = new ApplicationUser();

  public urlAddress: string = 'https://localhost:7231/Authentication/';
  constructor(private http: HttpClient, private route: ActivatedRoute, private router: Router, private notificationService: NotificationsService) { }

  GetUserData(UserName: string, Password: any) {
    debugger
    return this.http.get(this.urlAddress + 'GetUserData/' + UserName + "/" + Password).subscribe
      (
        (res: any) => {
          this.ApplicationUser = res as ApplicationUser;
          let obj = {
            UserName: this.ApplicationUser.UserName
          }
          if (!this.ApplicationUser.IsAuthenticated) {
            this.notificationService.ShowNotification(NotificationType.Error, "Username Or Password is Wrong");
            return;
          }

          sessionStorage.setItem("userData", JSON.stringify(obj));

          sessionStorage.setItem("token", this.ApplicationUser.Token as string);
          if (this.ApplicationUser.UserName !== undefined) {

            this.router.navigate(['/']);
            return true;
          }
          else {
            this.router.navigate(['/login']);
            return true;
          }

        }

      );
  }

}


