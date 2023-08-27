import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpResponse } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { catchError, map } from "rxjs/operators";
import { Router } from "@angular/router";
import { HelperService } from "../helper/helper.service";
import { NotificationsService } from "../Notifications/Notifications.service";
import { NotificationType } from "../Notifications/NotificationType";



@Injectable()
export class AuthInterceptor implements HttpInterceptor {

  constructor(private helper: HelperService, private router: Router, private notificationService: NotificationsService) {

  }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (sessionStorage.getItem('token') || this.helper.getCookie('token')) {
      const clonedReq = req.clone({
        headers: req.headers.set('Authorization', 'Bearer ' + sessionStorage.getItem('token'))
      });
      return next.handle(clonedReq)
        .pipe(catchError((err) => {
          if (err.status == 419) {
            sessionStorage.removeItem('token');
            this.router.navigateByUrl('/login');
          }
          else if (err.status == 401) {
            this.notificationService.ShowNotification(NotificationType.Error, "You Are Not Authorized");
          }

          return err;
        }))
        .pipe(map((evt: any) => {

          return evt;
        }));
    }

    else {
      return next.handle(req)
        .pipe(catchError((err) => {
          if (err.status == 419) {
            this.notificationService.ShowNotification(NotificationType.Error, "Session Timeout");
            sessionStorage.removeItem('token');
            this.router.navigateByUrl('/login');
          }
          else if (err.status == 401) {
            this.notificationService.ShowNotification(NotificationType.Error, "You Are Not Authorized");
          }
          return err;
        }))
        .pipe(map((evt: any) => {

          return evt;
        }));
    }
  }
}
