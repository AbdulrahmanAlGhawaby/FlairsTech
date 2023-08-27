import { Component } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { AuthenticationService } from '../Security/authentication.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {

  loginForm: FormGroup;
  constructor(private formBuilder: FormBuilder,
    private _AuthenticationService: AuthenticationService) {
    this.loginForm = this.formBuilder.group({
      UserName: new FormControl('', Validators.maxLength(6)),
      Password: new FormControl('', Validators.maxLength(6)),
      Branch: new FormControl('')
    });
  }

  onSubmit(UserData: any) {
    this._AuthenticationService.GetUserData(UserData.UserName, UserData.Password);

  }
}
