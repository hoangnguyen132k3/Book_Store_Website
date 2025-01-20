import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { NavbarComponent } from '../../../../core/component/navbar/navbar.component';
import { FooterComponent } from '../../../../core/component/footer/footer.component';
import { Router } from '@angular/router';
import { AuthService } from '../../../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, NavbarComponent, FooterComponent],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
})
export class LoginComponent {
  loginForm: FormGroup;
  errorMessage: string = '';

  constructor(private fb: FormBuilder, private router: Router, private authService: AuthService) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
    });
  }

  onSubmit() {
    if (this.loginForm.valid) {
      const { email, password } = this.loginForm.value;

      // Gọi AuthService để đăng nhập
      this.authService.login(email, password).subscribe({
        next: (response) => {
          console.log('Login successful:', response);

          // Lưu token và thời gian hết hạn
          this.authService.saveToken(response.token, response.expiresAt);

          // Lưu thông tin người dùng bao gồm userId và email
          this.authService.saveUser({
            userId: response.userId, // Đảm bảo userId được lấy từ API
            email: response.email,
          });

          console.log('User ID saved:', response.userId);

          // Điều hướng về trang chủ
          this.router.navigate(['/']);
        },
        error: (err) => {
          console.error(err);
          this.errorMessage = 'Đăng nhập thất bại. Vui lòng kiểm tra thông tin!';
        },
      });
    } else {
      this.errorMessage = 'Thông tin đăng nhập không hợp lệ!';
    }
  }

  goToRegister() {
    this.router.navigate(['/register']);
  }
}
