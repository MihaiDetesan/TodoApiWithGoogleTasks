using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Services;
using Google.Apis.Tasks.v1;
using Google.Apis.Tasks.v1.Data;
using OpenTelemetry.Trace;

namespace TodoApi
{
    public static class GoogleTasksExtensions
    {
        public static IServiceCollection AddGoogleTasksService(this IServiceCollection services)
        {
            // Wire up the token service
            return services.AddSingleton<IGoogleTasksApi, GoogleTasksApi>();
        }
    }

    public interface IGoogleTasksApi
    {
        //CRUD for lists
        Task<TodoList> CreateListAsync(string listName);
        Task<IEnumerable<TodoList>> GetListsAsync();
        Task<TodoList> GetListAsync(string listId);
        System.Threading.Tasks.Task<TodoList> UpdateListAsync(TodoList list);
        System.Threading.Tasks.Task<string> DeleteListAsync(string listId);

        // CRUD for tasks.
        Task<IList<Todo>> GetTasksAsync(string listId);
        Task<Todo?> GetTaskAsync(string listId, string taskId);
        Task<string> CreateTaskAsync(string listId, string taskTitle, string? parent = null);
        System.Threading.Tasks.Task DeleteTaskAsync(string listId, int taskId);
        Task<Todo> UpdateTaskAsync(string listId, string taskId, Todo toDo);
    }
    public class GoogleTasksApi: IGoogleTasksApi
    {
        private string[] Scopes = {
        TasksService.Scope.Tasks,
        TasksService.Scope.TasksReadonly  
        };

        private readonly GoogleOptions options;
        private TasksService tasksService;

        public GoogleTasksApi(GoogleOptions options)
        {
            this.options = options;
            var credentials = CreateCredentials(options.CredentialsPath, options.TokenPath);
            _ = credentials ?? throw new UnauthorizedAccessException("Could not authenticate to Google Tasks");
            tasksService = GetTasksService(credentials);
        }


        // CRUD for lists

        public async Task<IEnumerable<TodoList>> GetListsAsync()
        {
            var todoLists = new List<TodoList>();

            try
            {
                var listRequest = tasksService.Tasklists.List();
                var googleTaskLists = await listRequest.ExecuteAsync();
                googleTaskLists.Items.ToList().ForEach(x => todoLists.Add(TodoList.FromGoogleTaskList(x)));
                return todoLists;

            }
            catch(Exception _)
            {
                throw;
            }
        }

        public async Task<TodoList> CreateListAsync(string listName)
        {
            try
            {
                var googleTaskList = new TaskList()
                {
                    Title = listName,
                };
                
                var createRequest = tasksService.Tasklists.Insert(googleTaskList);
                var googleTaskLists = await createRequest.ExecuteAsync();
                var todoList = TodoList.FromGoogleTaskList(googleTaskLists);
                return todoList;
            }
            catch (Exception _)
            {
                throw;
            }
        }

        public async Task<TodoList> GetListAsync(string listId)
        { 
            try
            {
                var getRequest = tasksService.Tasklists.Get(listId);
                var googleTaskList = await getRequest.ExecuteAsync();
                return TodoList.FromGoogleTaskList(googleTaskList);   
            }
            catch (Exception _)
            {
                throw;
            }
        }

        public async System.Threading.Tasks.Task<TodoList> UpdateListAsync(TodoList list)
        {
            throw new NotImplementedException();
        }

        public async Task<string> DeleteListAsync(string listId)
        {
            try
            {
                var deleteRequest = tasksService.Tasklists.Delete(listId);
                return await deleteRequest.ExecuteAsync();
            }
            catch (Exception _)
            {
                throw;
            }
        }


        public async Task<IList<Todo>> GetTasksAsync(string listId)
        {
            var todos = new List<Todo>();

            try
            {
                var listRequest = tasksService.Tasks.List(listId);
                Tasks googleTasks;

                do
                {
                    googleTasks = listRequest.Execute();
                    googleTasks.Items.ToList().ForEach(x => todos.Add(Todo.FromGoogleTask(x)));
                    listRequest.PageToken = googleTasks.NextPageToken;
                } while (googleTasks.NextPageToken != null);
                //MTEwOTkwMjU4Mjc3MjUzMDY0MDY6MDow

                return todos;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<string> CreateTaskAsync( string listId, string title, string? parent = null)
        {
            try
            {
                var googleTask = new Google.Apis.Tasks.v1.Data.Task()
                {
                    Title = title,
                    Deleted = false,
                    Parent = parent ?? listId,
                };

                var request = tasksService.Tasks.Insert(googleTask, listId);
                var task = await request.ExecuteAsync();

                return task.Id;
                //MTEwOTkwMjU4Mjc3MjUzMDY0MDY6MDow                
            }
            catch (Exception _)
            {
                throw;
            }

        }

        public async Task<Todo?> GetTaskAsync(string listId, string taskId)
        {
            throw new NotImplementedException();
        }

        public async System.Threading.Tasks.Task DeleteTaskAsync(string listId, int taskId)
        {
            throw new NotImplementedException();
        }

        public async Task<Todo> UpdateTaskAsync(string listId, string taskId, Todo toDo)
        {
            throw new NotImplementedException();
        }   
        



        private UserCredential CreateCredentials(string credentialsPath, string tokenPath)
        {
            UserCredential credentials = null;
            using (var stream =
            new FileStream(credentialsPath, FileMode.Open, FileAccess.Read))
            {
                var secrets = GoogleClientSecrets.Load(stream).Secrets;
                var codeFlowInitializer = new GoogleAuthorizationCodeFlow.Initializer
                {
                    ClientSecrets = secrets,
                    Scopes = Scopes,
                };

                var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    codeFlowInitializer,
                    new[] { TasksService.Scope.Tasks },
                    "user",
                    CancellationToken.None).Result;

                return credential;
            }
        }

        /// <summary>
        /// Gets the calendar service.
        /// </summary>
        /// <param name="credentials"></param>
        /// <returns></returns>
        private TasksService GetTasksService(UserCredential credentials)
        {
            if (credentials == null)
                return null;

            var service = new TasksService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credentials,
                ApplicationName = options.ApplicationName,
            });

            return service;
        }

        //private IList<EventDay> GetEventsFromGoogle(CalendarService calendarService, DateTime startTime, DateTime endTime)
        //{
        //    var events = new List<EventDay>();

        //    foreach (var cal in Calendars)
        //    {
        //        var eventsInCalendar = GetEventsFromCalendar(cal.Id, calendarService, startTime, endTime).Items;

        //        foreach (var ev in eventsInCalendar)
        //        {
        //            try
        //            {
        //                var eventDay = new EventDay()
        //                {
        //                    Id = ev.Id,
        //                    Calendar = cal.Summary,
        //                    CalendarBkColor = cal.BackgroundColor,
        //                    CalendarId = cal.Id,
        //                    Description = ev.Summary,
        //                    StartDate = DateTime.Parse(ev.Start.Date),
        //                    EndDate = DateTime.Parse(ev.End.Date),
        //                };

        //                events.Add(eventDay);                        
                        
        //                if (vacationWords.Any(s=> eventDay.Description.IndexOf(s,StringComparison.InvariantCultureIgnoreCase) !=-1))
        //                {
        //                    if (CommittedHolidayDaysPerYear.ContainsKey(eventDay.StartDate.Year))
        //                    {
        //                        CommittedHolidayDaysPerYear[eventDay.StartDate.Year]+=(eventDay.EndDate - eventDay.StartDate).Days;
        //                    }
        //                    else
        //                    {
        //                        CommittedHolidayDaysPerYear[eventDay.StartDate.Year] = (eventDay.EndDate - eventDay.StartDate).Days;
        //                    }
        //                }
        //            }
        //            catch (Exception)
        //            {
        //                Console.WriteLine($"Id:{ev.Id}");
        //                Console.WriteLine($"StartDate:{ev.Start.Date}");
        //                Console.WriteLine($"EndDate:{ev.End.Date}");
        //                Console.WriteLine($"Description:{ev.Summary}");
        //            }
        //        }
        //    }

        //    return events;
        //}


    }
}

