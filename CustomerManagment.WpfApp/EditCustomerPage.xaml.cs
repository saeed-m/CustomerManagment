using CustomerManagment.WpfApp.Data;
using CustomerManagment.WpfApp.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Windows;

namespace CustomerManagment.WpfApp
{
    public partial class EditCustomerPage : Window
    {
        private readonly int _customerId;
        private readonly AppDbContext _context;
        private Customer? _customer;

        public EditCustomerPage(int customerId)
        {
            InitializeComponent();
            _customerId = customerId;
            _context = new AppDbContext();
            LoadCustomerData();
        }

        private void LoadCustomerData()
        {
            try
            {
                _customer = _context.Customers
                    .FirstOrDefault(c => c.Id == _customerId);

                if (_customer != null)
                {
                    // Load current customer data into text boxes
                    txtName.Text = _customer.CustomerName;
                    txtEmail.Text = _customer.CustomerEmail;
                    txtMobile.Text = _customer.CustomerMobileNumber ?? "";
                    txtPhone.Text = _customer.CustomerPhoneNumber ?? "";
                    txtJobTitle.Text = _customer.CustomerJobTitle ?? "";
                    txtAddress.Text = _customer.CustomerAddress ?? "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading customer data: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

            return System.Text.RegularExpressions.Regex.IsMatch(phone, @"^[\d\s\-\+\(\)]+$");
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
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

                if (_customer != null)
                {
                    // Update customer details
                    _customer.CustomerName = txtName.Text.Trim();
                    _customer.CustomerEmail = txtEmail.Text.Trim();
                    _customer.CustomerMobileNumber = string.IsNullOrWhiteSpace(txtMobile.Text) ? null : txtMobile.Text.Trim();
                    _customer.CustomerPhoneNumber = string.IsNullOrWhiteSpace(txtPhone.Text) ? null : txtPhone.Text.Trim();
                    _customer.CustomerJobTitle = string.IsNullOrWhiteSpace(txtJobTitle.Text) ? null : txtJobTitle.Text.Trim();
                    _customer.CustomerAddress = string.IsNullOrWhiteSpace(txtAddress.Text) ? null : txtAddress.Text.Trim();
                    _customer.UpdateDate = DateTime.UtcNow;

                    _context.SaveChanges();

                    MessageBox.Show("Customer updated successfully!", "Success",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    DialogResult = true;
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating customer: {ex.Message}", "Error",
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