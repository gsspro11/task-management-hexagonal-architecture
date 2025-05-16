import {} from '@coreui/angular';
import { ActivatedRoute } from '@angular/router';
import { Component, OnInit } from '@angular/core';
import { NgIf } from '@angular/common';
import { AlertComponent } from '@coreui/angular';
import { IconDirective } from '@coreui/icons-angular';
import { UserService } from '../../../core/services/user.service';
import { ValidationFormsService } from '../validation-forms.service';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  FormsModule,
  Validators,
  AbstractControl,
  ValidationErrors,
} from '@angular/forms';

import {
  FormFeedbackComponent,
  GutterDirective,
  ContainerComponent,
  RowComponent,
  ColComponent,
  TextColorDirective,
  CardComponent,
  CardBodyComponent,
  FormDirective,
  InputGroupComponent,
  InputGroupTextDirective,
  FormControlDirective,
  ButtonDirective,
} from '@coreui/angular';

/** passwords must match - custom validator */
export class PasswordValidators {
  static confirmPassword(control: AbstractControl): ValidationErrors | null {
    const password = control.get('password');
    const confirm = control.get('confirmPassword');
    if (password?.valid && password?.value === confirm?.value) {
      confirm?.setErrors(null);
      return null;
    }
    confirm?.setErrors({ passwordMismatch: true });
    return { passwordMismatch: true };
  }
}

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss'],
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
    TextColorDirective,
    InputGroupComponent,
    InputGroupTextDirective,
    IconDirective,
    FormsModule,
    GutterDirective,
    AlertComponent,
  ],
})
export class RegisterComponent implements OnInit {
  id: any;
  title: string;
  formErrors: any;
  submitted = false;
  messageError = '';
  messageSuccess = '';
  simpleForm!: FormGroup;
  formControls!: string[];
  customStylesValidated = false;

  constructor(
    private userService: UserService,
    private formBuilder: FormBuilder,
    private activedRoute: ActivatedRoute,
    public validationFormsService: ValidationFormsService
  ) {
    this.title = 'Create';
  }

  ngOnInit(): void {
    this.formErrors = this.validationFormsService.errorMessages;
    this.createForm();

    this.activedRoute.paramMap.subscribe((params) => {
      this.id = params.get('id');
    });

    if (this.id) {
      this.title = 'Update';
      this.fillUserInformation();
    }
  }

  createForm() {
    this.simpleForm = this.formBuilder.group(
      {
        firstName: ['', [Validators.required]],
        lastName: ['', [Validators.required]],
        email: ['', [Validators.required, Validators.email]],
        password: [
          '',
          [
            Validators.required,
            Validators.minLength(
              this.validationFormsService.formRules.passwordMin
            ),
            Validators.pattern(
              this.validationFormsService.formRules.passwordPattern
            ),
          ],
        ],
        confirmPassword: [
          '',
          [
            Validators.required,
            Validators.minLength(
              this.validationFormsService.formRules.passwordMin
            ),
            Validators.pattern(
              this.validationFormsService.formRules.passwordPattern
            ),
          ],
        ],
      },
      { validators: [PasswordValidators.confirmPassword] }
    );

    this.formControls = Object.keys(this.simpleForm.controls);
  }

  onReset() {
    this.submitted = false;
    this.simpleForm.reset();
  }

  onValidate() {
    this.messageError = '';
    this.messageSuccess = '';
    this.submitted = true;

    // stop here if form is invalid
    return this.simpleForm.status === 'VALID';
  }

  onSubmit() {
    if (this.onValidate()) {
      // TODO: Submit form value
      let firstName = this.simpleForm.value['firstName'];
      let lastName = this.simpleForm.value['lastName'];
      let email = this.simpleForm.value['email'];
      let password = this.simpleForm.value['password'];

      if (this.id) {
        this.edit(firstName, lastName, email, password);
      } else {
        this.register(firstName, lastName, email, password);
      }
    }
  }

  private edit(firstName: any, lastName: any, email: any, password: any) {
    this.userService
      .edit(this.id, { firstName, lastName, email, password })
      .subscribe({
        next: () => {
          this.messageError = '';
          this.messageSuccess = 'User updated successfully.';
          this.onReset();
        },
        error: (e) => {
          this.messageSuccess = '';
          this.messageError = e.message;
        },
      });
  }

  private register(firstName: any, lastName: any, email: any, password: any) {
    this.userService
      .register({ firstName, lastName, email, password })
      .subscribe({
        next: () => {
          this.messageError = '';
          this.messageSuccess = 'User registered successfully.';
          this.onReset();
        },
        error: (e) => {
          this.messageSuccess = '';
          this.messageError = e.message;
        },
      });
  }

  private fillUserInformation() {
    this.userService.get(this.id).subscribe((user) => {
      this.simpleForm.patchValue({
        firstName: user.firstName,
        lastName: user.lastName,
        email: user.email,
      });
    });
  }
}
