using System.ComponentModel;

namespace Client
{
    // @sckaushal: Issue 279: Added description attribute to display in status
    public enum SortType
    {
        [Description("---")]
        None,
        Alphabetical,
        Completed,
        Context,
        [Description("Due Date")]
        DueDate,
        Priority,
        Project,
        [Description("Order in file")]
        OrderInFile,
        [Description("Creation Date")]
        Created,
        Threshold
    }
}
