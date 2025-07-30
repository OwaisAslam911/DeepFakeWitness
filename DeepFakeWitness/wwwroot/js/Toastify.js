function showToastifyMessage(Message = null, ErrorType = null) {
 
    let backgroundColor = (ErrorType === 'success') ? 'green' : 'red';

    let toastClass = '';

    if (ErrorType === 'success') {
        toastClass = 'bg-success';
        
    } else {
        toastClass = 'bg-danger';
    }

    if (!Message) {
        Message = 'An unknown error occurred.';
    }

    if (typeof Message !== 'string') {
        Message = Message.toString();
    }

    let messageContainer = document.createElement('div');

    let shortMessage = Message.length > 40 ? Message.substring(0, 40) + "..." : Message;
    let shortMessageSpan = document.createElement('span');
    shortMessageSpan.innerHTML = shortMessage;

    let fullMessageSpan = document.createElement('span');
    fullMessageSpan.innerHTML = Message;
    fullMessageSpan.style.display = 'none';

    let dotsSpan = document.createElement('span');
    dotsSpan.innerHTML = '...';
    dotsSpan.style.display = shortMessage === Message ? 'none' : 'inline';

    let toggleLink = document.createElement('a');
    toggleLink.href = '#';
    toggleLink.innerHTML = 'Read more';
    toggleLink.style.marginLeft = '10px';
    toggleLink.style.cursor = 'pointer';

    messageContainer.appendChild(shortMessageSpan);
    messageContainer.appendChild(dotsSpan);
    messageContainer.appendChild(fullMessageSpan);
    messageContainer.appendChild(toggleLink);

    toggleLink.addEventListener('click', function (e) {
        e.preventDefault();
        if (dotsSpan.style.display === 'none') {
            dotsSpan.style.display = 'inline';
            fullMessageSpan.style.display = 'none';
            shortMessageSpan.style.display = 'inline';
            toggleLink.innerHTML = 'Read more';
        } else {
            dotsSpan.style.display = 'none';
            fullMessageSpan.style.display = 'inline';
            shortMessageSpan.style.display = 'none';
            toggleLink.innerHTML = 'Read less';
        }
    });

    Toastify({
        node: messageContainer,
        close: false,
        stopOnFocus: true,
        duration: 3000,
        className: toastClass,

        backgroundColor: backgroundColor ,
        position: "center"
    }).showToast();
}