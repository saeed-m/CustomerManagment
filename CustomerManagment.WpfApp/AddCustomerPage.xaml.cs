using CustomerManagment.WpfApp.Data;
using CustomerManagment.WpfApp.Data.Models;
using System.Text.RegularExpressions;
using System.Windows;

namespace CustomerManagment.WpfApp
{
    public partial class AddCustomerPage : Window
    {
        private readonly AppDbContext _context;

        public AddCustomerPage()
        {
            InitializeComponent();
            _context = new AppDbContext();
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPhoneNumber(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return true;

            // Simple phone validation - adjust as needed
            return Regex.IsMatch(phone, @"^[\d\s\-\+\(\)]+$");
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    MessageBox.Show("Please enter customer name.", "Validation Error",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtEmail.Text) || !IsValidEmail(txtEmail.Text))
                {
                    MessageBox.Show("Please enter a valid email address.", "Validation Error",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!string.IsNullOrWhiteSpace(txtMobile.Text) && !IsValidPhoneNumber(txtMobile.Text))
                {
                    MessageBox.Show("Please enter a valid mobile number.", "Validation Error",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Create new customer
                var customer = new Customer
                {
                    CustomerName = txtName.Text.Trim(),
                    CustomerEmail = txtEmail.Text.Trim(),
                    CustomerMobileNumber = string.IsNullOrWhiteSpace(txtMobile.Text) ? null : txtMobile.Text.Trim(),
                    CustomerPhoneNumber = string.IsNullOrWhiteSpace(txtPhone.Text) ? null : txtPhone.Text.Trim(),
                    CustomerJobTitle = string.IsNullOrWhiteSpace(txtJobTitle.Text) ? null : txtJobTitle.Text.Trim(),
                    CustomerAddress = string.IsNullOrWhiteSpace(txtAddress.Text) ? null : txtAddress.Text.Trim(),
                    CreateDate= DateTime.Now,
                };

                _context.Customers.Add(customer);
                _context.SaveChanges();

                MessageBox.Show("Customer added successfully!", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving customer: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}