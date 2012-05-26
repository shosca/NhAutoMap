namespace nhautomapprog.Models {
	public class Contact : Address // Base-type as an inheritance strategy
	{
		public virtual string Email { set; get; }

		public virtual string Mobile { set; get; }

		public virtual string Phone { set; get; }
	}
}
