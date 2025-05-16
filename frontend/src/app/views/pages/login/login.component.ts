import {} from '@coreui/angular';
import { Router } from '@angular/router';
import { Component } from '@angular/core';
import { NgIf, NgStyle } from '@angular/common';
import { AlertComponent } from '@coreui/angular';
import { IconDirective } from '@coreui/icons-angular';
import { UserService } from '../../../core/services/user.service';
import { AuthService } from '../../../auth/services/auth.service';
import { TokenService } from '../../../core/services/token.service';
import { ValidationFormsService } from '../validation-forms.service';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  FormsModule,
  Validators,
} from '@angular/forms';
import {
  FormFeedbackComponent,
  GutterDirective,
  RowDirective,
  ContainerComponent,
  RowComponent,
  ColComponent,
  CardGroupComponent,
  TextColorDirective,
  CardComponent,
  CardBodyComponent,
  FormDirective,
  InputGroupComponent,
  InputGroupTextDirective,
  FormControlDirective,
  ButtonDirective,
} from '@coreui/angular';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
  imports: [
    RowComponent,
    ColComponent,
    ReactiveFormsModule,
    FormDirective,
    NgIf,
    FormControlDirective,
    FormFeedbackComponent,
    ButtonDirective,
    CardComponent,
    CardBodyComponent,
    ContainerComponent,
    CardGroupComponent,
    TextColorDirective,
    InputGroupComponent,
    InputGroupTextDirective,
    IconDirective,
    NgStyle,
    FormsModule,
    RowDirective,
    GutterDirective,
    AlertComponent
  ],
})
export class LoginComponent {
  formErrors: any;
  submitted = false;
  messageError = null;
  simpleForm!: FormGroup;
  formControls!: string[];
  customStylesValidated = false;

  constructor(
    private router: Router,
    private authService: AuthService,
    private tokenService: TokenService,
    private userService: UserService,
    private formBuilder: FormBuilder,
    public validationFormsService: ValidationFormsService
  ) {
    if (this.authService.currentUserValue) this.router.navigate(['/base/board']);

    this.formErrors = this.validationFormsService.errorMessages;
    this.createForm();
  }


  createForm() {
    this.simpleForm = this.formBuilder.group({
      username: ['', [Validators.required]],
      password: ['', [Validators.required]],
    });
    this.formControls = Object.keys(this.simpleForm.controls);
  }

  onValidate() {
    this.submitted = true;

    // stop here if form is invalid
    return this.simpleForm.status === 'VALID';
  }

  onSubmit() {
    if (this.onValidate()) {
      // TODO: Submit form value
      const username = this.simpleForm.value['username'];
      const password = this.simpleForm.value['password'];

      this.userService.login(username, password).subscribe({
        next: (e) => {
            this.tokenService.store = e;
          this.authService.setAuthFromLocalStorage(e);
          this.authService.getUserByToken();
          this.router.navigate(['/base/board']);
        },
        error: (e) => (this.messageError = e.message),
      });
    }
  }
}
