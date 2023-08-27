import { Injectable } from '@angular/core';
import { Router, NavigationStart } from '@angular/router';
import { BehaviorSubject, Observable, Subject } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { NotificationType } from './NotificationType';

@Injectable({
  providedIn: 'root'
})
export class NotificationsService {
  public subject = new BehaviorSubject<any>([]);
  public keepAfterRouteChange = true;

  constructor(public router: Router, private _toastr: ToastrService) {
    // clear alert messages on route change unless 'keepAfterRouteChange' flag is true
    router.events.subscribe(event => {
      if (event instanceof NavigationStart) {
        if (this.keepAfterRouteChange) {
          // only keep for a single route change
          this.keepAfterRouteChange = false;
        } else {
          // clear alert messages
          this.clear();
        }
      }
    });
  }
  //this services is used for show notifiaction using toastr services
  //show dialog when you want to delete item to confirm before delete

  ShowNotification(_NotificationType: NotificationType, message?: any) {
    debugger;

    switch (_NotificationType) {
      case NotificationType.Success:
        this.success(message);
        break;
      case NotificationType.Error:
        this.error(message);
        break;
      case NotificationType.Info:
        this.info(message);
        break;
    }

  }
  getAlert(): Observable<any> {
    return this.subject.asObservable();
  }

  //start alertjs function
  private success(message: string) {
    debugger;
    this._toastr.success(message, '', { positionClass: 'toast-top-right' });
  }

  private error(message: string) {
    this._toastr.error(message, '', { positionClass: 'toast-top-right' });
  }

  private info(message: string) {
    this._toastr.info(message, '', { positionClass: 'toast-top-right' });
  }
 
  //end alertjs function

  clear() {
    this.subject.next([]);
  }
  //------------------------------------------


}

